namespace FakeItEasy.Core
{
    using System;

    internal sealed class CallCountConstraint
    {
        private readonly Func<int, bool> predicate;
        private readonly string description;

        public CallCountConstraint(Func<int, bool> predicate, string description)
        {
            this.predicate = predicate;
            this.description = description;
        }

        public bool Matches(int callCount)
        {
            try
            {
                return this.predicate.Invoke(callCount);
            }
            catch (Exception ex)
            {
                throw new UserCallbackException(ExceptionMessages.UserCallbackThrewAnException($"Call count constraint <{this.description}>"), ex);
            }
        }

        public override string ToString()
        {
            return this.description;
        }
    }
}
